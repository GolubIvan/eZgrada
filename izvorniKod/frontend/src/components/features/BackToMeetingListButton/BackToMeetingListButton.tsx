import { IconButton, Text } from "@chakra-ui/react";
import { useParams, useRouter } from "next/navigation";
import { BiChevronLeft } from "react-icons/bi";

export const BackToMeetingListButton = () => {
  const router = useRouter();
  const params = useParams();
  return (
    <IconButton
      bg="gray.300"
      variant="ghost"
      onClick={() => {
        router.push(`/building/${Number(params.zgradaId)}`);
      }}
    >
      <BiChevronLeft />
      <Text>{"Natrag na popis sastanka"}</Text>
    </IconButton>
  );
};